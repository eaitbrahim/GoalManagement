import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { BehavioralSkill } from './../../../_models/behavioralSkill';
import { BehavioralSkillEditModalComponent } from '../behavioral-skill-edit-modal/behavioral-skill-edit-modal.component';

@Component({
  selector: 'app-behavioral-skill-list',
  templateUrl: './behavioral-skill-list.component.html',
  styleUrls: ['./behavioral-skill-list.component.css']
})
export class BehavioralSkillListComponent implements OnInit {
  @Input() statusList: string[];
  @Input() behavioralSkills: BehavioralSkill[];
  @Output() loadBehavioralSkillsEvent = new EventEmitter<any>();
  @Output() editBehavioralSkillEvent = new EventEmitter<any>();
  filters: any = {};
  creationMode = false;
  bsModalRef: BsModalRef;
  dataType: string;

  constructor(
    private modalService: BsModalService
  ) { }

  ngOnInit() {
    this.dataType = 'behavioralSkill';
    this.loadBehavioralSkillsEvent.emit(this.filters);
  }

  resetFilters() {
    this.filters.status = '';
    this.loadBehavioralSkillsEvent.emit(this.filters);
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
      this.loadBehavioralSkillsEvent.emit(this.filters);
    }
  }

  handleLoadBehavioralSkills(event: any) {
    this.filters = event;
    this.loadBehavioralSkillsEvent.emit(this.filters);
  }


  handleCreationMode(event: boolean) {
    this.creationMode = event;
  }

  editBehavioralSkillModal(behavioralSkill: BehavioralSkill) {
    const initialState = {
      behavioralSkill,
      statusList: this.statusList
    };

    this.bsModalRef = this.modalService.show(BehavioralSkillEditModalComponent, { initialState, ignoreBackdropClick: true });
    this.bsModalRef.content.updateSelectedBehavioralSkill.subscribe((updatedBehavioralSkill) => {
      let updateParams = { updatedBehavioralSkill, filters: this.filters }
      this.editBehavioralSkillEvent.emit(updateParams);
    });
  }
}
