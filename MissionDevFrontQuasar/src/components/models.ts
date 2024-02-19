export type TRole = 'ADMIN' | 'WORKER';

export interface ITokenDecode {
  UserId: IUser['id'];
  UserFullname: IUser['fullname'];
  UserRoles: IUser['roles'];
  exp: number;
}

export interface IUser {
  id: string;
  idres: string;
  fullname: string;
  roles: TRole[];
}

export interface IProject {
  id: number;
  title: string;
  description: string;
  state: number;
  deadline: string;
}

export interface IZimbraMailPreview {
  s: number;
  d: number; // Timestamp
  l: string;
  cid: string;
  f: 'u' | 'a' | 'af' | 'ar' | 'arw' | 'r' | 's' | null;
  rev: number;
  id: string;
  su: string; // Subject
  fr: string; // Content preview
  cm: boolean;
  e: IZimbraMailExpeditor[];
  sf: string;
}

export interface IZimbraMail {
  s: number;
  d: number; // Timestamp
  l: string;
  cid: string;
  rev: number;
  id: string;
  fr: string; // Content preview
  e: IZimbraMailExpeditor[];
  su: string;
  mid: string;
  sd: number;
  mp: IZimbraMailMp[];
}

interface IZimbraMailMp {
  part: string;
  ct: string;
  s: number;
  body: boolean;
  content: string;
  cd: string;
  filename: string;
  ci: string;
  mp: IZimbraMailMp[];
}

export interface IZimbraMailExpeditor {
  a: string; // Email
  d: string; // Family Name
  p: string; // Full name
  t: string;
}
