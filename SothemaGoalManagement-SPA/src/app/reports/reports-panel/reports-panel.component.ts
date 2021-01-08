import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap';
import { ActivatedRoute } from '@angular/router';

import { Pagination, PaginatedResult } from '../../_models/pagination';
import { ReportEvaluation, ReportSheet } from '../../_models/report';
import { ReportGoal } from '../../_models/report';
import { HrService } from './../../_services/hr.service';
import { AlertifyService } from './../../_services/alertify.service';
import { Pole } from './../../_models/pole';
import { UserStatus } from 'src/app/_models/userStatus';

@Component({
  selector: 'app-reports-panel',
  templateUrl: './reports-panel.component.html',
  styleUrls: ['./reports-panel.component.css'],
})
export class ReportsPanelComponent implements OnInit {
  @ViewChild('tabset') tabset: TabsetComponent;
  sheetsPagination: Pagination;
  goalsPagination: Pagination;
  tabIndex = 0;
  public loading = false;
  sheets: ReportSheet[];
  goals: ReportGoal[];
  evaluations: ReportEvaluation[];
  filters: {
    year: string,
    userToSearch: string,
    poleId: number,
    pageSize: number,
    userStatusId: number
  } = {year: '0', userToSearch: '', poleId: 0, pageSize: 5, userStatusId: 0};
  yearList: number[] = [];
  poleList: Pole[] = [];
  userStatusList: UserStatus[];
  pageSizeList: number[] = [10, 50, 100, 500];

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
      this.goals = resolvedData['goals'].result;
      this.poleList = resolvedData['poleList'];
      this.userStatusList = resolvedData['userStatusList'];
      this.yearList = resolvedData['yearList'];
      this.sheetsPagination = resolvedData['sheets'].pagination;
      this.goalsPagination = resolvedData['goals'].pagination;
      this.buildEvaluations();
    });
  }

  buildEvaluations(){
    this.evaluations = [];
    this.evaluations = this.sheets.map(s => {
      return {
        fullName: s.fullName,
        employeeNumber: s.employeeNumber,
        year: s.year,
        goalsTotalGrade: s.goalsTotalGrade,
        behavioralSkillsGrade: Number(s.behavioralSkillsGrade) ? Number(s.behavioralSkillsGrade).toFixed(2) : '0.00'
      };
    });
  }

  handlePageChangedForSheets(event: any): void {
    this.sheetsPagination.currentPage = event.currentPage;
    this.loadSheets();
  }

  handlePageChangedForGoals(event: any): void {
    this.goalsPagination.currentPage = event.currentPage;
    this.loadGoals();
  }

  loadSheets() {
    this.loading = true;
    this.hrService
      .getReportSheets(
        this.sheetsPagination.currentPage,
        this.sheetsPagination.itemsPerPage,
        this.filters
      )
      .subscribe(
        (res: PaginatedResult<ReportSheet[]>) => {
          this.loading = false;
          this.sheets = res.result;
          this.buildEvaluations();
          this.sheetsPagination = res.pagination;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadGoals() {
    this.loading = true;
    this.hrService
      .getReportGoals(
        this.goalsPagination.currentPage,
        this.goalsPagination.itemsPerPage,
        this.filters
      )
      .subscribe(
        (res: PaginatedResult<ReportGoal[]>) => {
          this.loading = false;
          this.goals = res.result;
          this.goalsPagination = res.pagination;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadData() {
    this.loadSheets();
    this.loadGoals();
  }

  resetFilters() {
    this.setFilters();
    this.loadData();
  }

  setFilters(){
    this.filters.year = '0';
    this.filters.userToSearch = '';
    this.filters.poleId = 0;
    this.filters.pageSize = 10;
    this.filters.userStatusId = 0;
  }
}
