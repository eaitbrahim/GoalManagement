import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ReportEvaluation } from 'src/app/_models/report';

import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-reports-evaluations',
  templateUrl: './reports-evaluations.component.html',
  styleUrls: ['./reports-evaluations.component.css']
})
export class ReportsEvaluationsComponent implements OnInit {

  @Input() evaluations: ReportEvaluation[];
  @Input() pagination: Pagination;
  @Output() pageChangedEvent = new EventEmitter<any>();


  constructor() {
  }

  ngOnInit() {
  }

  pageChanged(event: any): void {
    const pageParams = { currentPage: event.page };
    this.pageChangedEvent.emit(pageParams);
  }
}
