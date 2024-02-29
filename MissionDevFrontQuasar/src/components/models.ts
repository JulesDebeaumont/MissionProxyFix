import { ISidebarComponentData } from 'src/utils/sketch-sidebar';

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
  projectUsers?: IProjectUser[];
}

export interface IProject {
  id: number;
  title: string;
  description: string;
  state: number;
  deadline: string;
  projectUsers?: IProjectUser[];
}

export interface IProjectUser {
  id: number;
  projectId: number;
  project?: IProject;
  userId: string;
  user?: IUser;
  MailFolderName: string;
}

export interface IUserFile {
  id: number;
  filename: string;
  mimeType: string;
  createdAt: string;
  updatedAt: string;
  userId: number;
  user?: IUser;
}

export interface IProjectFiler {
  id: number;
  filename: string;
  mimeType: string;
  createAt: string;
  updatedAt: string;
  userId: number;
  user?: IUser;
  fromMailId: string | null;
  isShared: boolean;
  projectId: number;
  project?: IProject;
}

export interface ISketch {
  id: number;
  title: string;
  core: ISketchCore;
  isShared: boolean;
  authorId: number;
  author?: IUser;
  projectId: number;
  project?: IProject;
}

export interface ISketchCore {
  id: string;
  type: 'custom';
  data?: ISidebarComponentData;
  dragging: boolean;
  position: {
    x: number;
    y: number;
  };
}

export interface IPosition {
  x: number;
  y: number;
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
