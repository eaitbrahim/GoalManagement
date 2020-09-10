import { GoalType } from './goalType';
import { AxisInstance } from './axisInstance';
import { GoalEvaluation } from './goalEvaluation';

export interface Goal {
  id: number;
  description: string;
  weight: number;
  created: Date;
  goalType: GoalType;
  projectName: string;
  axisInstance: AxisInstance;
  status: string;
  completionRate: number;
  parentGoalId: number;
  comment?: string;
  goalEvaluations: GoalEvaluation[];
  goalGrade: string;
  cascaderFullName?: string;
  cascaderPhotoUrl?: string;
}
