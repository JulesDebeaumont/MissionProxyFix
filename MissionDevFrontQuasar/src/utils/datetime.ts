import { date } from 'quasar';

export function dateFormatDDMMYYYY(dateTime: string | Date): string {
  return date.formatDate(dateTime, 'DD/MM/YYYY');
}

export function dateFormatDDMMYYYYHHMM(dateTime: string | Date): string {
  return date.formatDate(dateTime, 'DD/MM/YYYY HH:mm');
}
