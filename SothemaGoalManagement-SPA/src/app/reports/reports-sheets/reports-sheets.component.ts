import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ReportSheet } from 'src/app/_models/reportSheet';

import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-reports-sheets',
  templateUrl: './reports-sheets.component.html',
  styleUrls: ['./reports-sheets.component.css']
})
export class ReportsSheetsComponent implements OnInit {
  @Input() sheets: ReportSheet[];
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
