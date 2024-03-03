import { date } from 'quasar';

export function dateFormatDDMMYYYY(dateTime: string | Date): string {
  return date.formatDate(dateTime, 'YYYY-MM-DD');
}

export function dateFormatDDMMYYYYHHMM(dateTime: string | Date): string {
  return date.formatDate(dateTime, 'YYYY-MM-DD HH:mm');
}
