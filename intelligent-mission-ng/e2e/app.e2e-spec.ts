import { IntelligentMissionNgPage } from './app.po';

describe('intelligent-mission-ng App', () => {
  let page: IntelligentMissionNgPage;

  beforeEach(() => {
    page = new IntelligentMissionNgPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
