import { BehavioralSkillEvaluation } from './behavioralSkillEvaluation';

export interface BehavioralSkillInstance {
  id: number;
  skill: string;
  definition: string;
  levelOne: string;
  levelTwo: string;
  levelThree: string;
  levelFour: string;
  levelOneGrade: number;
  levelTwoGrade: number;
  levelThreeGrade: number;
  levelFourGrade: number;
  levelOneDescription: string;
  levelTwoDescription: string;
  levelThreeDescription: string;
  levelFourDescription: string;
  createdByName: string;
  createdById: number;
  status: string;
  created: Date;
  behavioralSkillEvaluations: BehavioralSkillEvaluation[];
  behavioralSkillGrade: number;
  behavioralSkillLevel: string;

}
