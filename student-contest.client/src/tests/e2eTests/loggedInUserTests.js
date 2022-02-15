import { Selector } from 'testcafe';
import { useCookies } from './auth';
import { validUser } from '../data/inputData';

const pageUrl = 'http://localhost:3000/';

fixture`Main page login`.page(pageUrl).beforeEach(async () => {
	await useCookies();
});

test('auto login success then user greeting visible', async (t) => {
	await t
		.navigateTo(pageUrl)
		.expect(Selector('p').innerText)
		.eql(
			'Nice to see you again, ' + validUser.firstName + ' ' + validUser.lastName
		);
});

test('navigate to private route success', async (t) => {
	await t
		.navigateTo('../test')
		.expect(Selector('p').innerText)
		.eql('Some info');
});

test('navigate to non-existing page error', async (t) => {
	await t
		.navigateTo('../notexists')
		.expect(Selector('p').innerText)
		.contains('Ooops! The page');
});

test('cannot navigate to login', async (t) => {
	await t
		.navigateTo('../login')
		.expect(Selector('p').innerText)
		.eql(
			'Nice to see you again, ' + validUser.firstName + ' ' + validUser.lastName
		);
});
