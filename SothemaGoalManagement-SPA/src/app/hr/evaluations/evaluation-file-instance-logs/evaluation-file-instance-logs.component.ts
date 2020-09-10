import { Component, OnInit, Input } from '@angular/core';
import { EvaluationFileInstanceLog } from '../../../_models/evaluationFileInstanceLog';

@Component({
  selector: 'app-evaluation-file-instance-logs',
  templateUrl: './evaluation-file-instance-logs.component.html',
  styleUrls: ['./evaluation-file-instance-logs.component.css']
})
export class EvaluationFileInstanceLogsComponent implements OnInit {
  @Input() evaluationFileInstanceLogs: EvaluationFileInstanceLog[];
  filteredEvaluationFileInstanceLogs: EvaluationFileInstanceLog[];
  values: string;

  constructor() { }

  ngOnInit() {
    this.values = '';
    this.filteredEvaluationFileInstanceLogs = this.evaluationFileInstanceLogs;
  }

  onKeyUp(event) {
    this.values = event.target.value;
    this.filteredEvaluationFileInstanceLogs = this.evaluationFileInstanceLogs
                                                  .filter(efil => efil.title.toLowerCase().includes(this.values.toLowerCase()));
  }
}
