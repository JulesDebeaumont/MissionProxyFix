<script setup lang="ts">
import { api } from 'src/boot/axios';
import { useUserStore } from 'src/stores/user-store';
import { computed, ref } from 'vue';
import { IZimbraMailPreview, IZimbraMail } from 'src/components/models';
import { dateFormatDDMMYYYYHHMM } from 'src/utils/datetime';

// props
const propsComponent = defineProps<{
  projectId: string;
}>();

// consts
const userStore = useUserStore();

// refs
const isLoadingAuthRef = ref(false);
const isLoadingGetMailsRef = ref(false);
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
  } catch (error) {
    console.log(error);
  } finally {
    isLoadingAuthRef.value = false;
  }
}
async function getZimbraMailByFolder(folderName: string) {
  isLoadingGetMailsRef.value = true;
  try {
    const responseZimbraEmailPreview = await api.post(`GetMailsByFolder`, {
      FolderName: folderName,
      MailCount: 100,
      AuthToken: userStore.getZimbraAuthTokenInCookie,
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
  try {
    const responseZimbraEmailPreview = await api.post(`GetMailById`, {
      mailId,
      AuthToken: userStore.getZimbraAuthTokenInCookie,
    });
    mailRef.value = responseZimbraEmailPreview.data as IZimbraMail;
  } catch (error) {
    console.log(error);
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
          `src="${process.env.API_URL}Projects/GetZimbraMailAttachment?MailId=${mail.id}&Part=${part}&AuthToken=${userStore.getZimbraAuthTokenInCookie}"`
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
</script>

<template>
  <q-input label="Password" v-model="passwordInputRef" />

  <q-btn @click="getZimbraAuthToken" label="Zimbra Auth !" color="primary" />
  <q-btn
    @click="getZimbraMailByFolder('homepop')"
    label="Get Mails"
    color="primary"
  />
  <template v-if="isLoadingGetMailsRef === false">
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
              ? 'bg-light-blue-2'
              : mailPreview.id === mailRef?.id
              ? 'bg-primary'
              : 'bg-white'
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
        v-if="mailRef !== null && htmlMail !== null"
        class="col-6"
        v-html="formatMailHtmlForImages(htmlMail, mailRef)"
      ></div>
      {{ htmlMail ?? 'sdfsdf' }}
    </div>
  </template>

  <div v-show="isLoadingGetMailsRef" class="flex flex-center">
    <q-spinner color="primary" size="md" />
  </div>
</template>
