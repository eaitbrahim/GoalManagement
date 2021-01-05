import { Goal } from './goal';

export interface AxisInstance {
  id: number;
  title: string;
  description: string;
  poleName: string;
  poleWeight: number;
  userWeight: number;
  goals: [Goal];
}
