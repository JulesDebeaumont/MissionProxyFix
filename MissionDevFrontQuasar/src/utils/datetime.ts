import { date } from 'quasar';

export function dateFormat(dateTime: string | Date): string {
  return date.formatDate(dateTime, 'DD/MM/YYYY');
}
