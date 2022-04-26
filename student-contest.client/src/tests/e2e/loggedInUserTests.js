import { login } from './auth';
import { UserAccountPage, MainPage, PrivatePage, ErrorPage } from './pages';
import { validUser } from '../data/inputData';

const pageUrl = 'http://localhost:3000/';

fixture`Main page login`.page(pageUrl).beforeEach(async (t) => {
	await login(t);
});

test('auto login success then user greeting visible', async () => {
	await MainPage.navigateToUrl(pageUrl);
	await UserAccountPage.assertGreetingMsgText(validUser);
});

test('navigate to private route success', async () => {
	await MainPage.navigateToUrl('/test');
	await PrivatePage.assertInfoMsgText();
});

test('navigate to non-existing page error', async () => {
	await MainPage.navigateToUrl('../notexists');
	await ErrorPage.assertError404MsgText();
	await ErrorPage.assertGoToMainPageButtonText();
});

test('cannot navigate to login', async () => {
	await MainPage.navigateToUrl('/login');
	await UserAccountPage.assertGreetingMsgText(validUser);
});
