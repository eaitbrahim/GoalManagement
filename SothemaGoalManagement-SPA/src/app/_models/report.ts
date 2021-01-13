
export interface ReportSheet {
  id: number;
  fullName: string;
  year: number;
  poleName: string;
  goalsStatus: string;
  validatorValidationDateTime: Date;
  employeeNumber: string;
  goalsTotalGrade: string;
  behavioralSkillsGrade: string;
}

export interface ReportGoal {
  weight: number;
  poleWeight: number;
  goal: string;
  axisTitle: string;
  fullName: string;
  poleName: string;
  year: number;
}

export interface ReportEvaluation {
    fullName: string;
    employeeNumber: string;
    year: number;
    goalsTotalGrade: string;
    behavioralSkillsGrade: string;
}
