import { AxisPole } from './axisPole';

export interface Axis {
  id: number;
  strategyId: number;
  title: string;
  description: string;
  created: Date;
  axisPoles: AxisPole[];
}
