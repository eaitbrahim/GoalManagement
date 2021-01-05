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
  filters: any = {};
  yearList: number[] = [];
  poleList: Pole[] = [];
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
    this.filters.year = '0';
    this.filters.userToSearch = '';
    this.filters.poleId = 0;
    this.route.data.subscribe((data) => {
      const resolvedData = data['resolvedData'];
      this.sheets = resolvedData['sheets'].result;
      this.buildGoals();
      this.poleList = resolvedData['poleList'];
      // this.notes = resolvedData['notes'].result;
      this.pagination = resolvedData['sheets'].pagination;

      for (const sheet of this.sheets) {
        if (!this.yearList.includes(sheet.year)) {
          this.yearList.push(sheet.year);
        }
      }
    });
  }

  buildGoals(){
    this.flattenedGoals = [];
    const goals = this.sheets.map(sheet => sheet.goals.map(goal => {
      return {
        goal: goal.description,
        weight: goal.weight,
        axisTitle: goal.axisInstance.title,
        poleName: goal.axisInstance.poleName,
        poleWeight: goal.axisInstance.poleWeight,
        year: sheet.year,
        fullName: sheet.fullName
      }
    }));
    this.flattenedGoals = [].concat.apply([],goals);
  }

  handlePageChanged(event: any): void {
    this.pagination.currentPage = event.currentPage;
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
    this.filters.year = '0';
    this.filters.userToSearch = '';
    this.filters.poleId = 0;
    this.loadData();
  }
}
