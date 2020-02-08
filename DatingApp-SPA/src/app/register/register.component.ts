import { Component, OnInit, enableProdMode, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { AuthService } from './../_services/auth.service';
import { AlertifyService } from './../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from 'src/app/_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();

  user: User;
  registerFrom: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private router: Router ,private alertify: AlertifyService, private fb: FormBuilder ) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
   this.createRegisterForm();
  }

  passwordMatchValidator(group: FormGroup) {
   return group.get('password').value === group.get('confirmPassword').value ? null : {mismatch : true};
  }

  createRegisterForm() {
    this.registerFrom = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(80)]],
      confirmPassword: ['', Validators.required]
    }, {validators: this.passwordMatchValidator});
  }

  register() {

    if (this.registerFrom.valid) {
          this.user = Object.assign({}, this.registerFrom.value);
          this.authService.register(this.user).subscribe(() =>{
            this.alertify.success('Registeration Successfull');
          },
          error => {
              this.alertify.error('register failed');
          }, () => {
            this.authService.login(this.user).subscribe(() => {
              this.router.navigate(['/members']);
            });
          }
          );
     }

  }

  cancel() {
        this.cancelRegister.emit(false);
  }

}
