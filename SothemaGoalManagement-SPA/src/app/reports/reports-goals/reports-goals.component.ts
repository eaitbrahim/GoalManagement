import { Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import { ReportSheet } from 'src/app/_models/reportSheet';

import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-reports-goals',
  templateUrl: './reports-goals.component.html',
  styleUrls: ['./reports-goals.component.css']
})
export class ReportsGoalsComponent implements OnInit {

  @Input() sheets: ReportSheet[];
  @Input() pagination: Pagination;
  @Output() pageChangedEvent = new EventEmitter<any>();
  flattenedGoals: {
        goal: string,
        weight: number,
        axisTitle: string,
        poleName: string,
        poleWeight: number,
        year: number,
        fullName: string
  }[] = [];

  constructor() {
  }

  ngOnInit() {
    const goals = this.sheets.map(sheet => sheet.goals.map(goal => {
      return {
        goal: goal.description,
        weight: goal.weight,
        axisTitle: goal.axisInstance.title,
        poleName: goal.axisInstance.poleName,
        poleWeight: goal.axisInstance.poleWeight,
        year: sheet.year,
        fullName: sheet.fullName
      }
    }));
    this.flattenedGoals = [].concat.apply([],goals);

  }

  pageChanged(event: any): void {
    const pageParams = { currentPage: event.page };
    this.pageChangedEvent.emit(pageParams);
  }

}
