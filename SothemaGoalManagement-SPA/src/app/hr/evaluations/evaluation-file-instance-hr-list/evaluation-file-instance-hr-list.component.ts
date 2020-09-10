import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faTrash } from '@fortawesome/free-solid-svg-icons';

import { EvaluationFileInstance } from '../../../_models/evaluationFileInstance';
import { UserStatus } from '../../../_models/userStatus';
import { User } from '../../../_models/user';

@Component({
  selector: 'app-evaluation-file-instance-hr-list',
  templateUrl: './evaluation-file-instance-hr-list.component.html',
  styleUrls: ['./evaluation-file-instance-hr-list.component.css']
})
export class EvaluationFileInstanceHrListComponent implements OnInit {
  @Input() evaluationFileInstanceList: EvaluationFileInstance[];
  @Output() deleteEvaluationFileInstanceEvent = new EventEmitter<EvaluationFileInstance>();
  @Output() loadLogsEvent = new EventEmitter();
  filteredEvaluationFileInstances: EvaluationFileInstance[];
  values: string;
  faTrash = faTrash;

  constructor() { }

  ngOnInit() {
    this.values = '';
    this.filteredEvaluationFileInstances = this.evaluationFileInstanceList;
  }


  onKeyUp(event) {
    this.values = event.target.value;
    this.filteredEvaluationFileInstances = this.evaluationFileInstanceList
                                                .filter(efi => efi.ownerName.toLowerCase()
                                                                            .includes(this.values.toLowerCase()));
  }

  delete(efi: EvaluationFileInstance) {
    this.deleteEvaluationFileInstanceEvent.emit(efi);
  }
}
