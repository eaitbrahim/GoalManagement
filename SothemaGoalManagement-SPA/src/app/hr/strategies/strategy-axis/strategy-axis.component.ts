import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { Axis } from './../../../_models/axis';
import { HrService } from '../../../_services/hr.service';
import { AlertifyService } from '../../../_services/alertify.service';
import { AuthService } from '../../../_services/auth.service';
import { AxisModalComponent } from '../axis-modal/axis-modal.component';

@Component({
  selector: 'app-strategy-axis',
  templateUrl: './strategy-axis.component.html',
  styleUrls: ['./strategy-axis.component.css']
})
export class StrategyAxisComponent implements OnInit {
  @Input() axisList: Axis[];
  @Output() addAxisEvent = new EventEmitter<Axis>();
  newAxis: any = {};
  bsModalRef: BsModalRef;

  constructor(private hrService: HrService, private authService: AuthService, private alertify: AlertifyService, private modalService: BsModalService) { }

  ngOnInit() {
  }

  addAxis() {
    this.addAxisEvent.emit(this.newAxis);
    this.newAxis.title = '';
    this.newAxis.description = '';
  }

  deleteAxis(id: number) {
    this.alertify.confirm(
      'Etes-vous sur de vouloir supprimer cet axe?',
      () => {
        this.hrService
          .deleteAxis(id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.axisList.splice(
                this.axisList.findIndex(a => a.id === id),
                1
              );
              this.alertify.success('L\'axe a été supprimé');
            },
            error => {
              this.alertify.error('Impossible de supprimer l\'axe');
            }
          );
      }
    );
  }

  editAxisModal(axis: Axis) {
    const initialState = {
      axis
    };

    this.bsModalRef = this.modalService.show(AxisModalComponent, { initialState });
    this.bsModalRef.content.updateSelectedAxis.subscribe((updatedAxis) => {
      this.hrService.updateAxis(axis.id, updatedAxis).subscribe(() => {
        this.alertify.success('L\'axe été mis à jour.');
      }, error => {
        this.alertify.error(error);
      })

    });
  }
}