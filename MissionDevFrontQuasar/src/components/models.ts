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
