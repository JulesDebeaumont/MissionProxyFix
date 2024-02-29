import { IProject } from 'src/components/models';

function getAllStates(): { [stateEnumIndex: number]: string } {
  return {
    0: 'En cours',
    1: 'En attente',
    2: 'Terminé',
    3: 'Annulé',
  };
}
export function getAllStatesOptions() {
  return Object.entries(getAllStates()).map((entry) => {
    return {
      label: entry[1],
      value: Number(entry[0]),
    };
  });
}
export function displayProjectState(project: IProject) {
  return getAllStates()[project.state];
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
