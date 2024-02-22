import { QBtn, QCard, QCardSection, QChip, QInput, QTable } from 'quasar';

interface IPropsTypeMap {
  [propsName: string]: 'boolean' | 'string' | 'number';
}
const Q_CHIPS_PROPS_TYPE_MAP: IPropsTypeMap = {
  dense: 'boolean',
  icon: 'string',
  color: 'string',
  textColor: 'string',
  selected: 'boolean',
  square: 'boolean',
  outline: 'boolean',
  clickable: 'boolean',
  removable: 'boolean',
  disable: 'boolean',
  ripple: 'boolean',
  size: 'string',
  label: 'string',
};
const Q_BUTTON_PROPS_TYPE_MAP: IPropsTypeMap = {
  size: 'string',
  disable: 'boolean',
  type: 'string',
  label: 'string',
  icon: 'string',
  flat: 'boolean',
  outline: 'boolean',
  unelevated: 'boolean',
  square: 'boolean',
  round: 'boolean',
  rounded: 'boolean',
  glossy: 'boolean',
  color: 'string',
  textColor: 'string',
  noCaps: 'boolean',
  dense: 'boolean',
  ripple: 'boolean',
  loading: 'boolean',
};
const Q_INPUT_PROPS_TYPE_MAP: IPropsTypeMap = {
  size: 'string',
  disable: 'boolean',
  type: 'string',
  label: 'string',
  icon: 'string',
  flat: 'boolean',
  outline: 'boolean',
  unelevated: 'boolean',
  square: 'boolean',
  round: 'boolean',
  rounded: 'boolean',
  glossy: 'boolean',
  color: 'string',
  textColor: 'string',
  noCaps: 'boolean',
  dense: 'boolean',
  ripple: 'boolean',
  loading: 'boolean',
};
const Q_TABLE_PROPS_TYPE_MAP: IPropsTypeMap = {
  size: 'string',
  disable: 'boolean',
  type: 'string',
  label: 'string',
  icon: 'string',
  flat: 'boolean',
  outline: 'boolean',
  unelevated: 'boolean',
  square: 'boolean',
  round: 'boolean',
  rounded: 'boolean',
  glossy: 'boolean',
  color: 'string',
  textColor: 'string',
  noCaps: 'boolean',
  dense: 'boolean',
  ripple: 'boolean',
  loading: 'boolean',
};
const Q_CARD_PROPS_TYPE_MAP: IPropsTypeMap = {
  square: 'boolean',
  flat: 'boolean',
  bordered: 'boolean',
};
const Q_CARD_SECTION_PROPS_TYPE_MAP: IPropsTypeMap = {
  horizontal: 'boolean',
};
export function getPropsTypeMapByComponent(component: any) {
  switch (component) {
    case QChip:
      return Q_CHIPS_PROPS_TYPE_MAP;
    case QBtn:
      return Q_BUTTON_PROPS_TYPE_MAP;
    case QInput:
      return Q_INPUT_PROPS_TYPE_MAP;
    case QTable:
      return Q_TABLE_PROPS_TYPE_MAP;
    case QCard:
      return Q_CARD_PROPS_TYPE_MAP;
    case QCardSection:
      return Q_CARD_SECTION_PROPS_TYPE_MAP;
    default:
      return {};
  }
}
