import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap';
import { ActivatedRoute } from '@angular/router';

import { Pagination, PaginatedResult } from '../../_models/pagination';
import { ReportSheet } from './../../_models/reportSheet';
import { HrService } from './../../_services/hr.service';
import { AlertifyService } from './../../_services/alertify.service';
import { Pole } from './../../_models/pole';

@Component({
  selector: 'app-reports-panel',
  templateUrl: './reports-panel.component.html',
  styleUrls: ['./reports-panel.component.css'],
})
export class ReportsPanelComponent implements OnInit {
  @ViewChild('tabset') tabset: TabsetComponent;
  pagination: Pagination;
  tabIndex = 0;
  public loading = false;
  sheets: ReportSheet[];
  filters: {
    year: string,
    userToSearch: string,
    poleId: number,
    pageSize: number
  } = {year: '0', userToSearch: '', poleId: 0, pageSize: 5};
  yearList: number[] = [];
  poleList: Pole[] = [];
  pageSizeList: number[] = [1];
  flattenedGoals: {
    goal: string,
    weight: number,
    axisTitle: string,
    poleName: string,
    poleWeight: number,
    year: number,
    fullName: string
}[] = [];

  constructor(
    private route: ActivatedRoute,
    private hrService: HrService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.setFilters();
    this.route.data.subscribe((data) => {
      const resolvedData = data['resolvedData'];
      this.sheets = resolvedData['sheets'].result;
      this.buildGoals();
      this.poleList = resolvedData['poleList'];
      this.yearList = resolvedData['yearList'];
      this.pageSizeList = [10, 20, 30, 40, 50, 100];
      this.pagination = resolvedData['sheets'].pagination;
    });
  }

  buildGoals(){
    this.flattenedGoals = [];

    const goals = this.sheets.map(sheet => sheet.extraInfoList.map(extraInfo => {
      return {
        goal: extraInfo.goal,
        weight: extraInfo.weight,
        axisTitle: extraInfo.axisTitle,
        poleName: sheet.poleName,
        poleWeight: extraInfo.poleWeight,
        year: sheet.year,
        fullName: sheet.fullName
      }
    }));
    this.flattenedGoals = [].concat.apply([],goals);
  }

  handlePageChanged(event: any): void {
    this.pagination.currentPage = event.currentPage;
    console.log('this.pagination.currentPage:', this.pagination.currentPage);
    this.loadSheets();
  }

  loadSheets() {
    this.loading = true;
    this.hrService
      .getReportSheets(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.filters
      )
      .subscribe(
        (res: PaginatedResult<ReportSheet[]>) => {
          this.loading = false;
          this.sheets = res.result;
          this.buildGoals();
          this.pagination = res.pagination;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadData() {
    this.loadSheets();
    // this.loadSheetsToValidate();
  }

  resetFilters() {
    this.setFilters();
    this.loadData();
  }

  setFilters(){
    this.filters.year = '0';
    this.filters.userToSearch = '';
    this.filters.poleId = 0;
    this.filters.pageSize = 5;
  }
}
