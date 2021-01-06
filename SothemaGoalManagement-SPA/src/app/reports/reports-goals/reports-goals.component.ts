import { Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import { ReportGoal } from 'src/app/_models/report';

import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-reports-goals',
  templateUrl: './reports-goals.component.html',
  styleUrls: ['./reports-goals.component.css']
})
export class ReportsGoalsComponent implements OnInit {

  @Input() goals: ReportGoal[];

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
