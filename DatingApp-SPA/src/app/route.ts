import { Routes } from '@angular/router';
import { Component } from '@angular/core';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_gaurds/auth.guard';
import { MemberDetailComponent } from './member-detail/member-detail.component';
import { MemberDetailRessolver } from './_ressolver/member-detail.ressolver';
import { MemberEditComponent } from './member-edit/member-edit.component';
import { MemberEditRessolver } from './_ressolver/member-edit.ressolver';
import { PreventUnsavedChanges } from './_gaurds/prevent-unsaved-changes.guard';
import { ListResolver } from './_ressolver/lists.resolver';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: '', runGuardsAndResolvers: 'always', canActivate: [AuthGuard], children: [
                {path: 'members', component: MemberListComponent},
                {path: 'members/edit', component: MemberEditComponent, resolve: {user: MemberEditRessolver},
                canDeactivate: [PreventUnsavedChanges]},
                {path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailRessolver}},
                {path: 'messages', component: MessagesComponent},
                {path: 'lists', component: ListsComponent , resolve: {user: ListResolver}}
            ]},
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
