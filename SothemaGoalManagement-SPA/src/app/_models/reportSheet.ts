
export interface ReportSheet {
  id: number;
  fullName: string;
  year: number;
  poleName: string;
  goalsStatus: string;
  validatorValidationDateTime: Date;
  extraInfoList: [ExtraInfo]
}

interface ExtraInfo {
  weight: number;
  poleWeight: number;
  goal: string;
  axisTitle: string;
}
