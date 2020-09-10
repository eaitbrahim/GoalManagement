import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { UserStatus } from '../../_models/userStatus';
import { Department } from './../../_models/department';

@Component({
  selector: 'app-admin-filter-actions',
  templateUrl: './admin-filter-actions.component.html',
  styleUrls: ['./admin-filter-actions.component.css']
})
export class AdminFilterActionsComponent implements OnInit {
  @Input() departmentList: Department[];
  @Input() userStatusList: UserStatus[];
  @Output() loadDataEvent = new EventEmitter<any>();
  @Output() creationModeEvent = new EventEmitter<any>();
  filters: any = {};

  constructor() { }

  ngOnInit() {
    this.filters.departmentId = 0;
    this.filters.userStatusId = 0;
    this.filters.userToSearch = '';
    this.filters.orderBy = 'lastActive';
  }

  resetFilters() {
    this.filters.departmentId = 0;
    this.filters.userStatusId = 0;
    this.filters.userToSearch = '';
    this.loadDataEvent.emit(this.filters);
  }

  creationToggle() {
    this.creationModeEvent.emit(true);
  }

  loadData() {
    this.loadDataEvent.emit(this.filters);
  }
}
