import { IProject } from 'src/components/models';

export function displayProjectState(project: IProject) {
  const stateDisplay: { [stateEnumIndex: number]: string } = {
    0: 'En cours',
    1: 'En attente',
    2: 'Terminé',
    3: 'Annulé',
  };
  return stateDisplay[project.state];
}
export function getColorClassByProjectState(project: IProject) {
  const stateDisplay: { [stateEnumIndex: number]: string } = {
    0: 'positive',
    1: 'warning',
    2: 'info',
    3: 'negative',
  };
  return stateDisplay[project.state];
}
