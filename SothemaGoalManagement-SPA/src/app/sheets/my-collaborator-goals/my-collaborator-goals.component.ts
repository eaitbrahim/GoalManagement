import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';

import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';
import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { PromptModalComponent } from '../../prompt-modal/prompt-modal.component';
import { Parameters } from '../../_models/parameters';
import { HrService } from '../../_services/hr.service';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-my-collaborator-goals',
  templateUrl: './my-collaborator-goals.component.html',
  styleUrls: ['./my-collaborator-goals.component.css']
})
export class MyCollaboratorGoalsComponent implements OnInit {
  @Input() goalsByAxisInstanceList: GoalByAxisInstance[];
  @Input() sheetToValidate: EvaluationFileInstance;
  @Output() switchOffGoalsEvent = new EventEmitter<boolean>();
  @Output() rejectGoalsEvent = new EventEmitter<any>();
  @Output() acceptGoalsEvent = new EventEmitter<any>();
  areGoalsReadOnly = true;
  canGoalsBeValidated = false;
  bsModalRef: BsModalRef;
  faArrowLeft = faArrowLeft;
  showDetail: boolean;
  parameters: Parameters[] = [];
  public loading = false;

  constructor(private modalService: BsModalService, private authService: AuthService, private hrService: HrService) { }

  ngOnInit() {
    this.loadParameters();
    if (this.goalsByAxisInstanceList[0].goalsStatus === 'En Revue') {
      this.canGoalsBeValidated = true;
    }
  }

  returnToSheets() {
    this.switchOffGoalsEvent.emit(false);
  }

  rejectGoals() {
    const initialState = {
      promptTitle: 'Renvoi',
      promptMessage: `Quelle est la raison de votre renvoi des objectifs de ${this.sheetToValidate.ownerName}?`
    };

    this.bsModalRef = this.modalService.show(PromptModalComponent, { initialState, ignoreBackdropClick: true, class: 'modal-lg' });
    this.bsModalRef.content.sendPromptValueEvent.subscribe((promptValue) => {
      const goals: any[] = [];
      this.goalsByAxisInstanceList.forEach(a => {
        a.goals.forEach(g => goals.push({
          id: g.id,
          description: g.description,
          goalTypeId: g.goalType.id,
          axisInstanceId: g.axisInstance.id,
          weight: g.weight,
          status: 'Rédaction',
          sheetTitle: this.sheetToValidate.title,
          emailContent: promptValue,
          sheetOwnerId: this.sheetToValidate.ownerId
        }));
      });

      const rejectionData = { sheetId: this.sheetToValidate.id, goals: goals };
      this.rejectGoalsEvent.emit(rejectionData);
      this.switchOffGoalsEvent.emit(false);
    });
  }

  acceptGoals() {
    const goals: any[] = [];
    this.goalsByAxisInstanceList.forEach(a => {
      a.goals.forEach(g => goals.push({
        id: g.id,
        description: g.description,
        goalTypeId: g.goalType.id,
        axisInstanceId: g.axisInstance.id,
        weight: g.weight,
        status: 'Publiée',
        sheetTitle: this.sheetToValidate.title,
        emailContent: `Les objectifs ont été acceptées pour la fiche ${this.sheetToValidate.title}.`,
        sheetOwnerId: this.sheetToValidate.ownerId
      }));
    });

    const acceptanceData = { sheetId: this.sheetToValidate.id, goals: goals };
    this.acceptGoalsEvent.emit(acceptanceData);
    this.switchOffGoalsEvent.emit(false);
  }

  CheckReadOnly() {
    const goalsInInitialStatus = this.goalsByAxisInstanceList.filter(
      (g) => g.goalsStatus === 'Pas encore créé' || g.goalsStatus === 'Rédaction'
    );

    if (goalsInInitialStatus.length === 0) {
      this.areGoalsReadOnly = false;
      console.log(
        '(goalsInInitialStatus) areGoalsReadOnly:',
        this.areGoalsReadOnly
      );
    }

    if (this.parameters.length > 0) {
        this.areGoalsReadOnly = this.checkEvents(`Plage de dates de validation des objectifs`);
        console.log(
          '(Validation date) areGoalsReadOnly:',
          this.areGoalsReadOnly
        );
    }

    if (this.authService.roleMatch(['HRD'])) {
      this.areGoalsReadOnly = false;
      console.log('(HRD) areGoalsReadOnly:', this.areGoalsReadOnly);
    }

    if (this.sheetToValidate.status === 'Publiée') {
      this.areGoalsReadOnly = true;
      console.log('(Published sheet) areGoalsReadOnly:', this.areGoalsReadOnly);
    }

    return this.areGoalsReadOnly;
  }

  loadParameters() {
    this.loading = true;
    this.hrService.loadParameters(this.sheetToValidate.evaluationFileId).subscribe(
      (result: Parameters[]) => {
        this.loading = false;
        this.parameters = result;

        this.CheckReadOnly();
      },
      (error) => {
        this.loading = false;
        console.log('Impossible de charger les événements.');
      }
    );
  }

  checkEvents(event) {
    const eventDateIdx = this.parameters.findIndex((p) => p.event.includes(event));

    if (eventDateIdx > -1) {
      const from = new Date(this.parameters[eventDateIdx].startEvent);
      const to = new Date(this.parameters[eventDateIdx].endEvent);
      to.setDate(to.getDate() + 1);
      const dateCheck = new Date();

      return dateCheck >= from && dateCheck < to;
    }

    return false;
  }
}
