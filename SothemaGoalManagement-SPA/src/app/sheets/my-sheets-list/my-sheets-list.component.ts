import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { Pagination } from '../../_models/pagination';

@Component({
  selector: 'app-my-sheets-list',
  templateUrl: './my-sheets-list.component.html',
  styleUrls: ['./my-sheets-list.component.css'],
})
export class MySheetsListComponent implements OnInit {
  @Input() sheets: EvaluationFileInstance[];
  @Input() pagination: Pagination;
  @Output() pageChangedEvent = new EventEmitter<any>();

  constructor() {}

  ngOnInit() {}

  pageChanged(event: any): void {
    const pageParams = { currentPage: event.page };
    this.pageChangedEvent.emit(pageParams);
  }
}
