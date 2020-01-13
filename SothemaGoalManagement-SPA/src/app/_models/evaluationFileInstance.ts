import { BehavioralSkillInstance } from "./behavioralSkillInstance";
import { AxisInstance } from "./axisInstance";

export interface EvaluationFileInstance {
  id: number;
  title: string;
  year: number;
  strategyTitle: string;
  strategyDescription: string;
  axisInstances: AxisInstance[];
  ownerName: string;
  ownerTitle: string;
  photoUrl: string;
  employeeNumber: string;
  ownerId: number;
  evaluationFileId: number
  status: string;
  goalsStatus: string
  created: Date;
  ownerComment: string;
  validatorComment: string;
  ownerValidationDateTime: Date;
  validatorValidationDateTime: Date;
  validatorId: number;
}
