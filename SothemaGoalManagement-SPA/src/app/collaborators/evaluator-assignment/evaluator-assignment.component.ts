import { Component, OnInit, Input } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { faTrash } from '@fortawesome/free-solid-svg-icons';

import { User } from '../../_models/user';
import { UserStatus } from '../../_models/userStatus';
import { AdminService } from '../../_services/admin.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Evaluator } from '../../_models/evaluator';
import { CollaboratorSearchComponent } from '../collaborator-search/collaborator-search.component';

@Component({
  selector: 'app-evaluator-assignment',
  templateUrl: './evaluator-assignment.component.html',
  styleUrls: ['./evaluator-assignment.component.css']
})
export class EvaluatorAssignmentComponent implements OnInit {
  @Input() evaluated: User;
  evaluators: Evaluator[];
  evaluatees: Evaluator[];
  public loading = false;
  bsModalRef: BsModalRef;
  userStatusList: UserStatus[];
  faTrash = faTrash;

  constructor(private modalService: BsModalService, private adminService: AdminService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.getUserStatus();
    this.loadEvaluators();
    this.loadEvaluatees();
  }

  getUserStatus() {
    if (localStorage.getItem('userStatusList')) {
      this.userStatusList = JSON.parse(localStorage.getItem('userStatusList'))
    } else {
      this.loading = true;
      this.adminService.getUserStatus().subscribe(
        (result: UserStatus[]) => {
          this.loading = false;
          this.userStatusList = result
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
    }
  }

  openModalForEvaluators() {
    const initialState = {
      userStatusList: this.userStatusList,
      actionLabel: 'Assigner comme évaluateur'
    };
    this.bsModalRef = this.modalService.show(CollaboratorSearchComponent, { initialState, ignoreBackdropClick: true, class: 'modal-lg' });
    this.bsModalRef.content.actionEvent.subscribe((users) => {
      this.loading = true;
      let userIds = users.map(u => u.id);
      this.adminService.addEvaluatorToUser(this.evaluated.id, userIds)
        .subscribe(
          () => {
            this.loading = false;
            this.loadEvaluators();
            this.alertify.success('Les évaluateurs ont été ajoutés avec succès');
          },
          error => {
            this.loading = false;
            this.alertify.error(error);
          }
        );
    });
  }

  openModalForEvaluatees() {
    const initialState = {
      userStatusList: this.userStatusList,
      actionLabel: 'Assigner comme évalué'
    };
    this.bsModalRef = this.modalService.show(CollaboratorSearchComponent, { initialState, ignoreBackdropClick: true, class: 'modal-lg' });
    this.bsModalRef.content.actionEvent.subscribe((users) => {
      this.loading = true;
      let userIds = users.map(u => u.id);
      this.adminService.addEvaluateeToUser(this.evaluated.id, userIds)
        .subscribe(
          () => {
            this.loading = false;
            this.loadEvaluatees();
            this.alertify.success('Les évalués ont été ajoutés avec succès');
          },
          error => {
            this.loading = false;
            this.alertify.error(error);
          }
        );
    });
  }

  handleUpdateRankOfEvaluator(event: Evaluator) {
    this.loading = true;
    this.adminService.updateRankOfEvaluator(this.evaluated.id, event.id, event.rank)
      .subscribe(
        next => {
          this.loading = false;
          this.alertify.success('Mise à jour du rang réussie');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadEvaluators() {
    this.loading = true;
    this.adminService.loadEvaluators(this.evaluated.id)
      .subscribe(
        (evaluators: Evaluator[]) => {
          this.loading = false;
          this.evaluators = evaluators;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadEvaluatees() {
    this.loading = true;
    this.adminService.loadEvaluatees(this.evaluated.id)
      .subscribe(
        (evaluatees: Evaluator[]) => {
          this.loading = false;
          this.evaluatees = evaluatees;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleDeleteEvaluator(event: Evaluator) {
    this.alertify.confirm('Supprimer',
      `Etes-vous sûr de vouloir supprimer l'évaluateur ${event.fullName}?`,
      () => {
        this.loading = true;
        this.adminService
          .deleteEvaluator(event.id, this.evaluated.id)
          .subscribe(
            () => {
              this.loading = false;
              this.loadEvaluators();
              this.alertify.success('L\'évaluateur a été supprimé.');
            },
            error => {
              this.loading = false;
              this.alertify.error('Échec de la suppression d\'évaluateur.');
            }
          );
      }
    );
  }

  deleteEvaluatee(evaluatee: Evaluator) {
    this.alertify.confirm('Supprimer',
      `Etes-vous sûr de vouloir supprimer l'évalué ${evaluatee.fullName}?`,
      () => {
        this.loading = true;
        this.adminService
          .deleteEvaluatee(evaluatee.id, this.evaluated.id)
          .subscribe(
            () => {
              this.loading = false;
              this.loadEvaluatees();
              this.alertify.success('L\'évalué a été supprimé.');
            },
            error => {
              this.loading = false;
              this.alertify.error('Échec de la suppression d\'évalué.');
            }
          );
      }
    );
  }
}
