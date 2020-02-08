import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from './../_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from './../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from './../_services/user.service';
import { AuthService } from './../_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
@ViewChild('editForm', {static: false}) editForm: NgForm;
user: User;
photoUrl: string;
// @HostListener('window:beforeunload', ['$event'])
// unloadNotification($event: any) {
//   if (this.editForm.dirty) {
//     // $event.returnValue = true;
//   }
// }

// this is to prevent unsaved changes when someone closes the browser as route gaurd cannot prevent it



  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
      console.log('befor edit');
      console.log(this.user);
    });

    this.authService.currentPhotoUrl.subscribe(p => this.photoUrl = p);
  }

updateUser() {
  console.log(this.user);
  this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(
    next => {
      this.alertify.success('Profile edit successfully');
      this.editForm.reset(this.user);
    },
    error => {
      this.alertify.error(error);
    }
  );
}

updateMainPhoto(photoUrl: string)
{
  this.user.photoUrl = photoUrl;
}

}
