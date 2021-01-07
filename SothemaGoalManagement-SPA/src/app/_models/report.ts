
export interface ReportSheet {
  id: number;
  fullName: string;
  year: number;
  poleName: string;
  goalsStatus: string;
  validatorValidationDateTime: Date;
  employeeNumber: string;
  goalsTotalGrade: number;
  behavioralSkillsGrade: number;
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
