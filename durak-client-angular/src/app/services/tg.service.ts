import { style } from '@angular/animations';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { InitResult, LaunchParams, init, parseLaunchParams, serializeLaunchParams } from '@tma.js/sdk';

@Injectable({
  providedIn: 'root'
})
export class TgService {
  private tgInstance: InitResult | null = null;

  public get tg(): InitResult {
    return this.tgInstance ? this.tgInstance : init();
  }

  constructor() {
  }

  async init() {
    try {
      this.tgInstance = await init({ cssVars: true, acceptCustomStyles: true, complete: true });
      this.tgInstance.viewport.expand();
    } catch (error) {
      console.error('Failed to initialize tg:', error);
    }
  }
}
