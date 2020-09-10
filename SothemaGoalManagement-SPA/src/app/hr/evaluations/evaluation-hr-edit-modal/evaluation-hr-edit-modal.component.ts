import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';

import { EvaluationFile } from '../../../_models/evaluationFile';
import { BehavioralSkill } from '../../../_models/behavioralSkill';
import { Strategy } from '../../../_models/strategy';

@Component({
  selector: 'app-evaluation-hr-edit-modal',
  templateUrl: './evaluation-hr-edit-modal.component.html',
  styleUrls: ['./evaluation-hr-edit-modal.component.css']
})
export class EvaluationHrEditModalComponent implements OnInit {
  @Output() updateSelectedEvaluationFile = new EventEmitter();
  evaluationFile: EvaluationFile;
  skillList: any[];
  strategyList: Strategy[];
  statusList: string[];
  showErrors: boolean;
  isReadOnly: boolean;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit() {
    if (this.evaluationFile.sealed) {
      this.isReadOnly = true;
      this.statusList = ['Publiée', 'Archivée'];
    }

  }

  updateEvaluationFile() {
    this.showErrors = false;
    const selectedSkillIds = [...this.skillList.filter(s => s.checked === true).map(s => s.id)]
    if (selectedSkillIds.length > 0) {
      const evaluationFileToUpdate = {
        id: this.evaluationFile.id,
        title: this.evaluationFile.title,
        year: this.evaluationFile.year,
        strategyId: this.evaluationFile.strategy.id,
        behavioralSkillIds: selectedSkillIds,
        status: this.evaluationFile.status
      };

      this.updateSelectedEvaluationFile.emit(evaluationFileToUpdate);
      this.bsModalRef.hide();
    } else {
      this.showErrors = true;
    }

  }
}
