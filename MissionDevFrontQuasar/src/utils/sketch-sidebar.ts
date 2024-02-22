import { QBtn, QCard, QCardSection, QChip } from 'quasar';

export interface ISidebarComponentData {
  type: any;
  props: { [propsLabel: string]: string | number | boolean };
  children: ISidebarComponentData[];
  rawContent?: string;
}

export const buttonComponents: ISidebarComponentData[] = [
  {
    type: QBtn,
    props: {
      color: 'red',
      label: 'Mommy',
    },
    children: [],
  },
];

export const chipComponents: ISidebarComponentData[] = [
  {
    type: QChip,
    props: {
      color: 'red',
      label: 'Mommy',
    },
    children: [],
  },
];

export const cardComponents: ISidebarComponentData[] = [
  {
    type: QCard,
    props: {
      bordered: true,
    },
    children: [
      {
        type: QCardSection,
        props: {},
        children: [
          {
            type: 'div',
            props: {
              class: 'text-h6',
            },
            rawContent: 'Our Changing Planet',
            children: [],
          },
        ],
      },
    ],
  },
];
