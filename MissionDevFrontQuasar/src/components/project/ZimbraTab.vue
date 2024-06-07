<script setup lang="ts">
import { api } from 'src/boot/axios';
import { useUserStore } from 'src/stores/user-store';
import { computed, ref, onMounted } from 'vue';
import { IZimbraMailPreview, IZimbraMail } from 'src/components/models';
import { dateFormatDDMMYYYYHHMM } from 'src/utils/datetime';

// props
const propsComponent = defineProps<{
  projectId: string;
}>();

// consts
const userStore = useUserStore();

// refs
const isLoadingGetFolder = ref(false);
const isLoadingAuthRef = ref(false);
const isLoadingGetMailsRef = ref(false);
const isLoadingGetMailRef = ref(false);
const setupFolderMode = ref(false);
const connexionMode = ref(false);
const currentUserFolderMail = ref<string | null>(null);
const mailsPreviewRef = ref<IZimbraMailPreview[] | null>(null);
const mailRef = ref<IZimbraMail | null>(null);
const passwordInputRef = ref('');

// functions
async function getZimbraAuthToken() {
  isLoadingAuthRef.value = true;
  try {
    const responseZimbraCrsf = await api.post(`MailBox/GetAuthToken`, {
      password: passwordInputRef.value,
    });
    userStore.setupZimbraAuthToken(responseZimbraCrsf.data as string);
    connexionMode.value = false;
    await getUserFolderMailForProject();
  } catch (error) {
    console.log(error);
  } finally {
    isLoadingAuthRef.value = false;
  }
}
async function getZimbraMail() {
  isLoadingGetMailsRef.value = true;
  try {
    const responseZimbraEmailPreview = await api.post(`MailBox/GetMails`, {
      projectId: propsComponent.projectId,
      mailCount: 100,
      authToken: userStore.getZimbraAuthTokenInCookie,
    });
    mailsPreviewRef.value =
      responseZimbraEmailPreview.data as IZimbraMailPreview[];
  } catch (error) {
    console.log(error);
  } finally {
    isLoadingGetMailsRef.value = false;
  }
}
async function getZimbraMailById(mailId: string) {
  isLoadingGetMailRef.value = true;
  try {
    const responseZimbraEmailPreview = await api.post(`MailBox/GetMailById`, {
      mailId,
      AuthToken: userStore.getZimbraAuthTokenInCookie,
    });
    mailRef.value = responseZimbraEmailPreview.data as IZimbraMail;
  } catch (error) {
    console.log(error);
  } finally {
    isLoadingGetMailRef.value = false;
  }
}
async function getUserFolderMailForProject() {
  isLoadingGetFolder.value = true;
  try {
    const responseUserFolder = await api.get(
      `Projects/${propsComponent.projectId}/GetFolderName`
    );
    currentUserFolderMail.value = responseUserFolder.data.mailFolderName;
    if (currentUserFolderMail.value === null) {
      setupFolderMode.value = true;
    } else {
      await getZimbraMail();
    }
  } catch (error) {
    console.log(error);
  } finally {
    isLoadingGetFolder.value = false;
  }
}
async function setUserFolderMailForProject() {
  isLoadingGetFolder.value = true;
  try {
    await api.put(`Projects/${propsComponent.projectId}/SetFolderName`, {
      folderName: currentUserFolderMail.value,
    });
    setupFolderMode.value = false;
    await getZimbraMail();
  } catch (error) {
    console.log(error);
  } finally {
    isLoadingGetFolder.value = false;
  }
}
function formatMailHtmlForImages(mailHtmlContent: string, mail: IZimbraMail) {
  const allSrcOccurences = mailHtmlContent.matchAll(/src="cid[^"]*"/g);
  Array.from(allSrcOccurences).forEach(async (occurence) => {
    if (occurence.index) {
      const partIdentifier = occurence[0].split('"').at(1)?.replace('cid:', '');
      if (partIdentifier) {
        const part = findPartInMpObject(partIdentifier, mail.mp);
        mailHtmlContent = mailHtmlContent.replace(
          occurence[0],
          `src="${process.env.API_URL}MailBox/GetAttachment?MailId=${mail.id}&Part=${part}&AuthToken=${userStore.getZimbraAuthTokenInCookie}&ProjectId=${propsComponent.projectId}&Filename=image.png"`
        );
      }
    }
  });
  mailHtmlContent = mailHtmlContent.replaceAll('dfsrc="', 'src="');
  const allDataMceSrcOccurences =
    mailHtmlContent.matchAll(/data-mce-src="[^"]*"/g);
  Array.from(allDataMceSrcOccurences).forEach(async (occurence) => {
    if (occurence.index) {
      mailHtmlContent = mailHtmlContent.replace(occurence[0], '');
    }
  });
  return mailHtmlContent;
}
function findPartInMpObject(
  identifier: string,
  mpObjectList: IZimbraMail['mp']
): null | string {
  let part =
    mpObjectList.find((mpObject) => {
      return (
        mpObject.ci !== undefined && mpObject.ci === '<' + identifier + '>'
      );
    })?.part ?? null;
  if (part === null) {
    for (let i = 0; i < mpObjectList.length; i++) {
      if (mpObjectList[i].mp) {
        part = findPartInMpObject(identifier, mpObjectList[i].mp);
        if (part !== null) {
          return part;
        }
      }
    }
  }
  return part;
}
function findBodyInMpObject(mpObjectList: IZimbraMail['mp']): null | string {
  let part =
    mpObjectList.find((mpObject) => {
      return mpObject.body !== undefined && mpObject.body === true;
    })?.content ?? null;
  if (part === null) {
    for (let i = 0; i < mpObjectList.length; i++) {
      if (mpObjectList[i].mp) {
        part = findBodyInMpObject(mpObjectList[i].mp);
        if (part !== null) {
          return part;
        }
      }
    }
  }
  return part;
}

// computeds
const htmlMail = computed(() => {
  if (mailRef.value === null) {
    return null;
  }
  const html = findBodyInMpObject(mailRef.value.mp);
  if (html !== null) {
    return formatMailHtmlForImages(html, mailRef.value);
  }
  return '';
});

// lifeCycle
onMounted(async () => {
  if (userStore.zimbraAuthTokenInCookie === false) {
    connexionMode.value = true;
  } else {
    await getZimbraMail();
  }
});
</script>

<template>
  <div v-if="connexionMode">
    Veuillez saisir votre mot de passe Zimbra (promis je le regarde pas).
    <q-form @submit="getZimbraAuthToken">
      <q-input
        label="Mot de passe"
        type="password"
        v-model="passwordInputRef"
      />
      <q-btn
        type="submit"
        label="Connexion"
        no-caps
        color="primary"
        class="q-mt-sm"
      />
    </q-form>
  </div>

  <div v-if="setupFolderMode">
    <div>
      Veuillez saisir le nom du dossier Zimbra auquel vous voulez associez le
      projet.
    </div>
    <q-form @submit="setUserFolderMailForProject">
      <q-input
        v-model="currentUserFolderMail"
        label="Nom du dossier Zimbra"
        filled
      />
      <q-btn
        type="submit"
        label="Enregistrer"
        no-caps
        color="primary"
        class="q-mt-sm"
      />
    </q-form>
  </div>

  <template
    v-if="
      isLoadingGetMailsRef === false &&
      setupFolderMode === false &&
      connexionMode === false
    "
  >
    <div class="flex row">
      <q-list
        v-if="mailsPreviewRef !== null"
        separator
        bordered
        class="bg-grey-2 q-pa-md col-6"
      >
        <q-item
          v-for="mailPreview in mailsPreviewRef"
          :key="mailPreview.id"
          clickable
          class="rounded-borders"
          :class="
            mailPreview.f === 'u'
              ? 'bg-grey-4'
              : mailPreview.id === mailRef?.id
              ? 'bg-green-2'
              : ''
          "
          @click="getZimbraMailById(mailPreview.id)"
        >
          <q-item-section>
            <q-item-label>{{ mailPreview.su }}</q-item-label>
            <q-item-label caption class="ellipsis">{{
              mailPreview.fr
            }}</q-item-label>
          </q-item-section>

          <q-item-section side top>
            <div>{{ dateFormatDDMMYYYYHHMM(new Date(mailPreview.d)) }}</div>
          </q-item-section>
        </q-item>
      </q-list>
      <div
        v-if="
          mailRef !== null && htmlMail !== null && isLoadingGetMailRef === false
        "
        class="col-6 q-pa-md"
        v-html="formatMailHtmlForImages(htmlMail, mailRef)"
      ></div>
      <div v-show="isLoadingGetMailRef" class="flex">
        <q-spinner color="primary" size="md" />
      </div>
    </div>
  </template>

  <div
    v-show="isLoadingGetMailsRef || isLoadingGetFolder"
    class="flex flex-center"
  >
    <q-spinner color="primary" size="md" />
  </div>
</template>
