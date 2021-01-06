import { Component, OnInit, Input, Output, EventEmitter} from '@angular/core';

import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-reports-goals',
  templateUrl: './reports-goals.component.html',
  styleUrls: ['./reports-goals.component.css']
})
export class ReportsGoalsComponent implements OnInit {

  @Input() flattenedGoals: {
    goal: string,
    weight: number,
    axisTitle: string,
    poleName: string,
    poleWeight: number,
    year: number,
    fullName: string
  }[];

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
