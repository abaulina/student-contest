import { RequestMock } from 'testcafe';
import { login } from './auth';
import { MainPage, ErrorPage } from './pages';
import configData from '../../utilities/config.json';

const url = configData.SERVER_URL + '/login';
const mock404 = RequestMock()
	.onRequestTo({
		url: url,
		method: 'POST'
	})
	.respond(null, 404, {
		'Access-Control-Allow-Origin': 'http://localhost:3000',
		'Access-Control-Allow-Credentials': true,
		'Content-type': 'application/json; charset=utf-8'
	});
const mock500 = RequestMock()
	.onRequestTo({
		url: url,
		method: 'POST'
	})
	.respond(null, 500, {
		'Access-Control-Allow-Origin': 'http://localhost:3000',
		'Access-Control-Allow-Credentials': true,
		'Content-type': 'application/json; charset=utf-8'
	});

fixture`Main page`.page`http://localhost:3000`;

test('navigate to non-existing page redirect to error page', async () => {
	await MainPage.navigateToUrl('../smthStupid');

	await ErrorPage.assertError404MsgText();
	await ErrorPage.assertGoToMainPageButtonText();
});

test.requestHooks(mock500)(
	'server respond with error other than 404 redirect to error page',
	async (t) => {
		await login(t);

		await ErrorPage.assertErrorDefaultMsgText();
		await ErrorPage.assertGoToMainPageButtonText();
	}
);

test.requestHooks(mock404)(
	'server respond with 404 redirect to not found page',
	async (t) => {
		await login(t);

		await ErrorPage.assertError404MsgText();
		await ErrorPage.assertGoToMainPageButtonText();
	}
);
