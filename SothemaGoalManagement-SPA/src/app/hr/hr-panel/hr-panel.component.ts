import { filter } from 'rxjs/operators';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TabsetComponent } from 'ngx-bootstrap';
import { Router } from '@angular/router';

import { Pagination, PaginatedResult } from '../../_models/pagination';
import { HrService } from '../../_services/hr.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { BehavioralSkill } from '../../_models/behavioralSkill';
import { Strategy } from '../../_models/strategy';
import { EvaluationFile } from '../../_models/evaluationFile';
import { EvaluationFileInstanceLog } from '../../_models/evaluationFileInstanceLog';


@Component({
  selector: 'app-hr-panel',
  templateUrl: './hr-panel.component.html',
  styleUrls: ['./hr-panel.component.css']
})
export class HrPanelComponent implements OnInit {
  @ViewChild('hrTabs') hrTabs: TabsetComponent;
  statusList: string[];
  strategies: Strategy[];
  behavioralSkills: BehavioralSkill[];
  evaluationFiles: EvaluationFile[];
  strategyList: Strategy[];
  skillList: BehavioralSkill[];
  pagination: Pagination;
  public loading = false;
  evaluationFileInstanceLogs: EvaluationFileInstanceLog[] = [];

  constructor(private route: ActivatedRoute, private hrService: HrService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.strategies = data['strategies'].result;
      this.pagination = data['strategies'].pagination;
    });

    this.statusList = ['Rédaction', 'En Revue', 'Publiée', 'Archivée'];

    // https://hackernoon.com/everything-you-need-to-know-about-the-expressionchangedafterithasbeencheckederror-error-e3fd9ce7dbb4
    Promise.resolve(null).then(() => {
      this.route.queryParams.subscribe(params => {
        const selectedTab = params['tab'] || 0;
        this.hrTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
      });
    });

    this.loadLogs();
  }

  loadLogs() {
    this.loading = true;
    this.hrService.getEvaluationSheetLogs().subscribe(
      (result: EvaluationFileInstanceLog[]) => {
        this.loading = false;
        this.evaluationFileInstanceLogs = result;
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  handleLoadStrategies(filters) {
    this.loading = true;
    this.hrService
      .getStrategies(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        filters
      )
      .subscribe(
        (res: PaginatedResult<Strategy[]>) => {
          this.loading = false;
          this.strategies = res.result;
          this.pagination = res.pagination;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleEditStrategy(event: any) {
    this.loading = true;
    this.hrService.updateStrategy(this.authService.decodedToken.nameid, event.updatedStrategy).subscribe(() => {
      this.loading = false;
      this.alertify.success('Stratégie a été mise à jour.');
      this.handleLoadStrategies(event.filters);
    }, error => {
      this.loading = false;
      this.handleLoadStrategies(event.filters);
      this.alertify.error(error);
    })
  }

  handleLoadBehavioralSkills(filters) {
    this.loading = true;
    this.hrService
      .getBehavioralSkills(this.authService.decodedToken.nameid, filters)
      .subscribe(
        (res: BehavioralSkill[]) => {
          this.loading = false;
          this.behavioralSkills = res;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleEditBehavioralSkill(event: any) {
    this.loading = true;
    this.hrService.updateBehavioralSkill(this.authService.decodedToken.nameid, event.updatedBehavioralSkill).subscribe(() => {
      this.loading = false;
      this.alertify.success('Compétence comportementale a été mise à jour.');
      this.handleLoadBehavioralSkills(event.filters);
    }, error => {
      this.loading = false;
      this.handleLoadBehavioralSkills(event.filters);
      this.alertify.error(error);
    });
  }

  handleLoadEvaluationFiles(filters) {
    this.loading = true;
    this.hrService
      .getEvaluationFiles(this.authService.decodedToken.nameid, filters)
      .subscribe(
        (res: EvaluationFile[]) => {
          this.loading = false;
          this.evaluationFiles = res;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleEditEvaluationFile(event: any) {
    this.loading = true;
    this.hrService.updateEvaluationFile(this.authService.decodedToken.nameid, event.updatedEvaluationFile).subscribe(() => {
      this.loading = false;
      this.alertify.success('Fiche d\'évaluation a été mise à jour.');
      this.handleLoadEvaluationFiles(event.filters);
      this.handleLoadStrategies(event.filters);
      this.handleLoadBehavioralSkills(event.filters);
    }, error => {
      this.loading = false;
      this.handleLoadEvaluationFiles(event.filters);
      this.handleLoadStrategies(event.filters);
      this.handleLoadBehavioralSkills(event.filters);
      this.alertify.error(error);
    })
  }

  handlePageChanged(event: any): void {
    this.pagination.currentPage = event.currentPage;
    this.handleLoadStrategies(event.filters);
  }

  handleLoadPublishedStratgeies() {
    this.loading = true;
    this.hrService.getPublishedStrategies().subscribe(
      (result: Strategy[]) => {
        this.loading = false;
        this.strategyList = result;
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  handleLoadPublishedBehavioralSkills() {
    this.loading = true;
    this.hrService.getPublishedBehavioralSkills().subscribe(
      (result: BehavioralSkill[]) => {
        this.loading = false;
        this.skillList = result;
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }
}
