import { Goal } from './goal';

export interface GoalByAxisInstance {
  axisInstanceId: number;
  title: string;
  description: string;
  poleName: string;
  poleWeight: number;
  userWeight: number;
  goalsStatus: string;
  goals: Goal[];
  totalGoals: number;
  totalGoalWeight: number;
  axisGrade: string;
  totalGrade: string;
}
