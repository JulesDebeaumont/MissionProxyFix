export type TRole = 'ADMIN' | 'WORKER';

export interface ITokenDecode {
  UserId: IUser['Id'];
  UserFullname: IUser['Fullname'];
  UserRoles: IUser['Roles'];
  exp: number;
}

export interface IUser {
  Id: string;
  Fullname: string;
  Roles: TRole[];
}

export interface IProject {
  ID: number;
  Title: string;
  Description: string;
}
