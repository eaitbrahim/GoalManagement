import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap';
import { ActivatedRoute } from '@angular/router';

import { Pagination, PaginatedResult } from '../../_models/pagination';
import { ReportSheet } from './../../_models/reportSheet';
import { HrService } from './../../_services/hr.service';
import { AlertifyService } from './../../_services/alertify.service';

@Component({
  selector: 'app-reports-panel',
  templateUrl: './reports-panel.component.html',
  styleUrls: ['./reports-panel.component.css']
})
export class ReportsPanelComponent implements OnInit {

  @ViewChild('tabset') tabset: TabsetComponent;
  pagination: Pagination;
  tabIndex = 0;
  public loading = false;
  sheets: ReportSheet[];

  constructor(private route: ActivatedRoute, private hrService: HrService, private alertify: AlertifyService) { }


  ngOnInit() {
    this.route.data.subscribe((data) => {
      const resolvedData = data['resolvedData'];
      this.sheets = resolvedData['sheets'].result;
      // this.notes = resolvedData['notes'].result;
      this.pagination = resolvedData['sheets'].pagination;
      console.log('sheets in panel:', this.sheets);
    });
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
        this.pagination.itemsPerPage
      )
      .subscribe(
        (res: PaginatedResult<ReportSheet[]>) => {
          this.loading = false;
          this.sheets = res.result;
          this.pagination = res.pagination;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }
}
