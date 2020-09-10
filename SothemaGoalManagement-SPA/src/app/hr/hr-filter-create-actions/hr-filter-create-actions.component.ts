import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-hr-filter-create-actions',
  templateUrl: './hr-filter-create-actions.component.html',
  styleUrls: ['./hr-filter-create-actions.component.css']
})
export class HrFilterCreateActionsComponent implements OnInit {
  @Input() dataType: string;
  @Input() statusList: string[];
  @Output() loadDataEvent = new EventEmitter<any>();
  @Output() creationModeEvent = new EventEmitter<any>();

  filters: any = {};
  labelButton: string;

  constructor() { }

  ngOnInit() {
    this.filters.status = '';
    this.filters.orderBy = 'created';
    switch (this.dataType) {
      case 'strategy':
        this.labelButton = 'Créer Nouveau Axe';
        break;
      case 'behavioralSkill':
        this.labelButton = 'Créer Nouvelle compétence comportementale';
        break;
      case 'evaluationFile':
        this.labelButton = 'Créer Nouveau Modèle d\'Evaluation';
        break;
    }
  }

  resetFilters() {
    this.filters.status = '';
    this.loadDataEvent.emit(this.filters);
  }

  creationToggle() {
    this.creationModeEvent.emit(true);
  }

  loadData() {
    this.loadDataEvent.emit(this.filters);
  }
}
