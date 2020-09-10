import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { Pagination } from '../../../_models/pagination';
import { Strategy } from '../../../_models/strategy';
import { StrategyEditModalComponent } from '../strategy-edit-modal/strategy-edit-modal.component';


@Component({
  selector: 'app-strategy-list',
  templateUrl: './strategy-list.component.html',
  styleUrls: ['./strategy-list.component.css']
})
export class StrategyListComponent implements OnInit {
  @Input() strategies: Strategy[];
  @Input() pagination: Pagination;
  @Input() statusList: string[];
  @Output() loadStrategiesEvent = new EventEmitter<any>();
  @Output() editStrategyEvent = new EventEmitter<any>();
  @Output() pageChangedEvent = new EventEmitter<any>();
  creationMode = false;
  bsModalRef: BsModalRef;
  dataType: string;
  filters: any = {};

  constructor(
    private modalService: BsModalService
  ) { }

  ngOnInit() {
    this.dataType = 'strategy';
  }

  pageChanged(event: any): void {
    const pageParams = { currentPage: event.page, filters: this.filters };
    this.pageChangedEvent.emit(pageParams);;
  }

  cancelCreationMode(creationMode: boolean) {
    this.creationMode = creationMode;
  }

  switchOffCreationMode(reload: boolean) {
    this.creationMode = false;
    if (reload) {
      this.loadStrategiesEvent.emit(this.filters);
    }
  }

  handleLoadStrategies(event: any) {
    this.filters = event;
    this.loadStrategiesEvent.emit(this.filters);
  }

  handleCreationMode(event: boolean) {
    this.creationMode = event;
  }

  editStrategyModal(strategy: Strategy) {
    const initialState = {
      strategy,
      statusList: this.statusList
    };

    this.bsModalRef = this.modalService.show(StrategyEditModalComponent, { initialState, ignoreBackdropClick: true });
    this.bsModalRef.content.updateSelectedStrategy.subscribe((updatedStrategy) => {
      const updateParams = { updatedStrategy, filters: this.filters };
      this.editStrategyEvent.emit(updateParams);
    });
  }
}
