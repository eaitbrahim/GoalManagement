import { Axis } from './axis';

export interface Strategy {
  id: number;
  title: string;
  description: string;
  ownerName: string;
  ownerId: number;
  status: string;
  created: Date;
  documentationUrl: string;
  axisList: Axis[];
  sealed?: boolean;
  sealedDate?: Date;
}
