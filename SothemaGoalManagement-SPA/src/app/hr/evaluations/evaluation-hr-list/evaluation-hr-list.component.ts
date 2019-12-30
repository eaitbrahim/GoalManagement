import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { EvaluationFile } from './../../../_models/evaluationFile';
import { EvaluationHrEditModalComponent } from '../evaluation-hr-edit-modal/evaluation-hr-edit-modal.component';
import { Strategy } from '../../../_models/strategy';
import { BehavioralSkill } from '../../../_models/behavioralSkill';

@Component({
  selector: 'app-evaluation-hr-list',
  templateUrl: './evaluation-hr-list.component.html',
  styleUrls: ['./evaluation-hr-list.component.css']
})
export class EvaluationHrListComponent implements OnInit {
  @Input() statusList: string[];
  @Input() evaluationFiles: EvaluationFile[];
  @Input() strategyList: Strategy[];
  @Input() skillList: BehavioralSkill[];
  @Output() loadEvaluationFilesEvent = new EventEmitter<any>();
  @Output() editEvaluationFileEvent = new EventEmitter<any>();
  @Output() loadPublishedStratgeiesEvent = new EventEmitter<any>();
  @Output() loadPublishedBehavioralSkillsEvent = new EventEmitter<any>();
  filters: any = {};
  creationMode = false;
  bsModalRef: BsModalRef;
  dataType: string;

  constructor(
    private modalService: BsModalService
  ) { }

  ngOnInit() {
    this.dataType = 'evaluationFile';
    this.loadEvaluationFilesEvent.emit(this.filters);
    this.loadPublishedBehavioralSkillsEvent.emit();
    this.loadPublishedStratgeiesEvent.emit();
  }

  resetFilters() {
    this.filters.status = '';
    this.loadEvaluationFilesEvent.emit(this.filters);
  }

  creationToggle() {
    this.creationMode = true;
  }

  cancelCreationMode(creationMode: boolean) {
    this.creationMode = creationMode;
  }

  switchOffCreationMode(reload: boolean) {
    this.creationMode = false;
    if (reload) {
      this.loadEvaluationFilesEvent.emit(this.filters);
      this.loadPublishedBehavioralSkillsEvent.emit();
      this.loadPublishedStratgeiesEvent.emit();
    }
  }

  handleLoadEvaluations(event: any) {
    this.filters = event;
    this.loadEvaluationFilesEvent.emit(this.filters);
  }


  handleCreationMode(event: boolean) {
    this.creationMode = event;
  }

  editEvaluationFileModal(evaluationFile: any) {
    const initialState = {
      evaluationFile,
      skillList: this.getSkillsArray(evaluationFile),
      strategyList: this.strategyList,
      statusList: this.statusList
    };

    this.bsModalRef = this.modalService.show(EvaluationHrEditModalComponent, { initialState, ignoreBackdropClick: true });
    this.bsModalRef.content.updateSelectedEvaluationFile.subscribe((updatedEvaluationFile) => {
      let updateParams = { updatedEvaluationFile: updatedEvaluationFile, filters: this.filters }
      this.editEvaluationFileEvent.emit(updateParams);
    });
  }

  private getSkillsArray(evaluationFile) {
    const skills = [];
    const availableSkills: any[] = [...this.skillList];

    for (let i = 0; i < availableSkills.length; i++) {
      let isMatch = false;
      for (let j = 0; j < evaluationFile.behavioralSkills.length; j++) {
        if (availableSkills[i].id === evaluationFile.behavioralSkills[j].id) {
          isMatch = true;
          availableSkills[i].checked = true;
          skills.push(availableSkills[i]);
          break;
        }
      }
      if (!isMatch) {
        availableSkills[i].checked = false;
        skills.push(availableSkills[i]);
      }
    }
    return skills;
  }
}

