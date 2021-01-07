import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ReportSheet } from 'src/app/_models/report';

import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-reports-evaluations',
  templateUrl: './reports-evaluations.component.html',
  styleUrls: ['./reports-evaluations.component.css']
})
export class ReportsEvaluationsComponent implements OnInit {

  @Input() sheets: ReportSheet[];
  @Input() pagination: Pagination;
  @Output() pageChangedEvent = new EventEmitter<any>();
  evaluations: {
    fullName: string,
    employeeNumber: string,
    year: number,
    goalsTotalGrade: number,
    behavioralSkillsGrade: string
  }[] = [];

  constructor() {
  }

  ngOnInit() {
    this.evaluations = this.sheets.map(s => {
      return {
        fullName: s.fullName,
        employeeNumber: s.employeeNumber,
        year: s.year,
        goalsTotalGrade: s.goalsTotalGrade,
        behavioralSkillsGrade: Number(s.behavioralSkillsGrade).toFixed(2)
      };
    });
  }

  pageChanged(event: any): void {
    const pageParams = { currentPage: event.page };
    this.pageChangedEvent.emit(pageParams);
  }
}
