import { AxisInstance } from './axisInstance';

export interface ReportSheet {
  id: number;
  fullName: string;
  year: number;
  poleName: string;
  goalsStatus: string;
  validatorValidationDateTime: Date;
  axisInstances: [AxisInstance]
}
