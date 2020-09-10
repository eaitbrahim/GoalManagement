import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';

import { BehavioralSkill } from '../../../_models/behavioralSkill';

@Component({
  selector: 'app-behavioral-skill-edit-modal',
  templateUrl: './behavioral-skill-edit-modal.component.html',
  styleUrls: ['./behavioral-skill-edit-modal.component.css']
})
export class BehavioralSkillEditModalComponent implements OnInit {
  @Output() updateSelectedBehavioralSkill = new EventEmitter();
  isFirstOpen = true;
  behavioralSkill: BehavioralSkill;
  statusList: string[];
  isReadOnly: boolean;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit() {
    if (this.behavioralSkill.sealed) {
      this.isReadOnly = true;
    }
  }

  updateBehavioralSkill() {
    this.updateSelectedBehavioralSkill.emit(this.behavioralSkill);
    this.bsModalRef.hide();
  }
}
