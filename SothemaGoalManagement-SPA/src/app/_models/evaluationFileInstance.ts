import { BehavioralSkillInstance } from "./behavioralSkillInstance";
import { AxisInstance } from "./axisInstance";

export interface EvaluationFileInstance {
  id: number;
  title: string;
  year: number;
  strategy: string;
  behavioralSkills: BehavioralSkillInstance[];
  axisInstances: AxisInstance[];
  ownerName: string;
  ownerId: number;
  status: string;
  created: Date;
}