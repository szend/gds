import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RootFilter } from '../Models/Parameters';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

 baseUrl: string = "https://localhost:7053/api";

 //baseUrl: string = "https://gds-bkbnb8b0dhaxf2hu.eastus-01.azurewebsites.net/api";

 // baseUrl: string = "https://gds-bkbnb8b0dhaxf2hu.eastus-01.azurewebsites.net/api";

  constructor(public http: HttpClient) { }

  AddToDashboard(dashtable : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DashboardTable/AddToDashboard/` , dashtable );
  }

  SaveDashboard(dashtable : string, size : number, position: number):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DashboardTable/SaveDashboard/` + dashtable + '/' + size+ '/' + position );
  }

  RemoveFromDashboard(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DashboardTable/RemoveFromDashboard/` + id );
  }

  AddToDashboardChart(chart : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Chart/AddToDashboard/` , chart );
  }

  AddToDashboardCharts(charts : any[]):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Chart/AddToDashboardArray/` , charts );
  }


  SaveChartDashboard(chart : string, size : number, position: number):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Chart/SaveChartDashboard/` + chart + '/' + size+ '/' + position );
  }

  RemoveFromDashboardChart(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Chart/RemoveFromDashboard/` + id );
  }

  Connect(db : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DatabaseConnectionProperty/Connect/` , db );
  }

  EditConnection(db : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DatabaseConnectionProperty/Edit/` , db );
  }
  EditConnectionAPI(db : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DatabaseConnectionProperty/Editapi/` , db );
  }

  GetTableNames(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/GetTableNames/`+ id );
  }

  ExecuteQuery(id: string, query : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/ExecuteQuery/`+ id + '/' + query );
  }


  GetDatabases():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/GetDatabases` );
  }

  GetAPIs():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/GetApis` );
  }

  GetPublicDatabases():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/GetPublicDatabases` );
  }

  ImportTables(id: string, alltable : string[]):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DatabaseConnectionProperty/ImportTables/`+ id,alltable );
  }

  
  ImportApiTables(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/ImportApiTables/`+ id );
  }

  RefreshTables(id: string, alltable : string[]):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DatabaseConnectionProperty/RefreshTables/`+ id,alltable );
  }
  DisconnectTables(id: string, alltable : string[]):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DatabaseConnectionProperty/DisconnectTables/`+ id,alltable );
  }
  DeleteDatabase(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DatabaseConnectionProperty/DeleteDatabase/`+ id );
  }

  GetConnectedTables(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetConnectedTables/`+ id );
  }
  
  GetDashboardData():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetDashboardData/` );
  }

  GetDashboardDataOther(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetDashboardDataOther/`+ id  );
  }

  GetAllType():Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/GetByFilter/` , null );
  }


  GetCount(rootfilter : RootFilter | undefined):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/GetCount/` , rootfilter );
  }

  GetTypeByFilter(rootfilter : RootFilter | undefined, privatelist: boolean = false):Observable<any> {
    if(privatelist){
      return this.http.post<any>(`${this.baseUrl}/ObjectType/GetByFilterPrivate/` , rootfilter );
    }
    else{
      return this.http.post<any>(`${this.baseUrl}/ObjectType/GetByFilter/` , rootfilter );
    }
  }

  GetAllPublicDashboard():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetAllPublicDashboard/` ); 
}

  GetTypeByChart(chart : any | undefined, privatelist: boolean = false, value : any,label : any):Observable<any> {
      return this.http.post<any>(`${this.baseUrl}/ObjectType/GetTypeByChart/`+privatelist+'/'+value+'/'+label , chart ); 
  }

  GetPage(rootfilter : RootFilter | undefined, id: string, name : string):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/GetPage/`+id + '/'+ name , rootfilter );
  }

  CreatePage(page : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/TablePage/Create/` , page );
  }

  EditPage(page : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/TablePage/Edit/` , page );
  }

  GetAllPage(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/TablePage/GetAllPage/` + id );
  }

  DeletePage(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/TablePage/Delete/` + id );
  }


  SaveObjectType(type: any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/Save/` , type );
  }

  AddChild(type: any, id : string):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/AddChild/`+id , type );
  }

  RemoveChild(childid : string, id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/RemoveChild/`+childid +'/'+id );
  }

  GetAllChild(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetAllChild/` + id );
  }
  
  SaveDataObject(type: any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DataObject/Save/` , type );
  }

  CreateDataObject(type: any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DataObject/Create/` , type );
  }

  EditAllFiltered(obj: any,filter : RootFilter):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/EditAllFiltered/` , { object:obj, filter: filter });
  }

  SelectChild(childid: string, parentid : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/SelectChild/`+ childid +'/'+ parentid );
  }

  DeleteList(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/Delete/`+ id );
  }
  DeleteObject(id: {key : string, value : string}[], typid : string | undefined):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/DataObject/Delete/`+typid, id );
  }

  GetToken(email : string, password : string):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Users/BearerToken`, {email : email, password: password});
  }

  CreateUser(username : string, password : string, email: string):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Users/`, {name : username, password: password, email: email});
  }

  ListOwner(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Users/ListOwner/`+ id );
  }

  SetDashoardPublic():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Users/SetDashoardPublic/`);
  }

  SetDashoardNonPublic():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Users/SetDashoardNonPublic/`);
  }


  IsDashboardPublic():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Users/IsDashboardPublic/`);
  }

  SetLimit(datacount : number, extdatacount : number, listcount : number, str : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Users/SetLimit/`+ datacount + "/" + extdatacount+"/" + listcount+"/" + str );
  }


  GetAllCategory():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetAllCategory/` );
  }

  GetChildTypeByFilter(rootfilter : RootFilter | undefined):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ObjectType/GetChildByFilter/` , rootfilter );
  }

  GetObjectAccess():Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/DataObject/GetAccess/`);
  }

  SaveFile(file :any, id : string,name: string,parentid: string):Observable<any> {
    return this.http.post<boolean>(`${this.baseUrl}/File/SaveFile?id=`+id+'&field='+name+'&table='+parentid,file);
  }

  DownloadFile(id : number):Observable<any> {
    return this.http.get(`${this.baseUrl}/File/Download?id=`+id,{responseType: 'blob'});
  }

  Logout():Observable<any> {
    return this.http.get(`${this.baseUrl}/Users/Logout/`);
  }

  
  GetMessages():Observable<any> {
    return this.http.get(`${this.baseUrl}/Message/GetMessages/`);
  }

  CreateMessage(message : any):Observable<any> {
    return this.http.post(`${this.baseUrl}/Message/Create/`,message);
  }

  DeleteMessage(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Message/Delete/`+ id );
  }

  GetListByObjectId(id: string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetByObjectId/`+ id );
  }

  GetUser(mail: string):Observable<any> {
    return this.http.get(`${this.baseUrl}/Users/`+ mail);
  }

  ChartData(rootfilter : RootFilter):Observable<any> {
      return this.http.post<any>(`${this.baseUrl}/ObjectType/GetChartData/` , rootfilter );
  }

  ChartDataBase(id : string):Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ObjectType/GetChartDataBase/`+ id );
}

SelectParent(id : string, name: string):Observable<any> {
  return this.http.get<any>(`${this.baseUrl}/ObjectType/SelectParent/`+ id +'/'+name);
}

  Import(file :any, id : string, parent : string | undefined | null):Observable<any> {
    if(parent == null || parent == undefined){
      return this.http.post<boolean>(`${this.baseUrl}/File/Import?id=`+id,file);
    }
    else{
      return this.http.post<boolean>(`${this.baseUrl}/File/Import?id=`+id+'&parent='+parent,file);
    }
  }

  ImportCreate(model : any):Observable<any> {
      return this.http.post<boolean>(`${this.baseUrl}/File/ImportCreate`,model);
  }

  GetSettingData():Observable<any> {
    return this.http.get(`${this.baseUrl}/Users/GetSettingData/`);
  }

  SendOffer(offer : any):Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Offer/Create/`, offer);
}

GetOffers():Observable<any> {
  return this.http.get<any>(`${this.baseUrl}/Offer/GetAllOffer/`);
}

Search(searchstring : string):Observable<any> {
  return this.http.get<any>(`${this.baseUrl}/ObjectType/Search?searchstring=`+searchstring);
}
SaveCalculatedField(calculatedfield : any):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/SaveCalculatedField`, calculatedfield);
}

SaveCalculatedColor(calculatedcolor : any):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/SaveCalculatedColor`, calculatedcolor);
}

SaveCalculatedColorLabel(calculatedcolor : any):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/SaveCalculatedColorLabel`, calculatedcolor);
}

SaveCalculatedSize(calculatedcolor : any):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/SaveCalculatedSize`, calculatedcolor);
}

CreateCalculatedChart(calculatedcolor : any):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/CreateCalculatedChart`, calculatedcolor);
}

ML(rootfilter : RootFilter | undefined):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/ML/` , rootfilter );
}

ImageClassification(rootfilter : RootFilter | undefined, name : string):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/ImageClassification/`+name , rootfilter );
}

ClassifySingleImage(id : string, name : string,keys: {key : string, value : string}[]):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/ClassifySingleImage/`+id +'/'+name,keys);
}

CreateRegression(rootfilter : RootFilter | undefined, name : string):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/CreateRegression/`+name , rootfilter );
}

PredictRegression(id : string, name : string,keys: {key : string, value : string}[]):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/PredictRegression/`+id +'/'+name,keys);
}

Classification(rootfilter : RootFilter | undefined,name : string):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/Classification/`+name , rootfilter );
}


PredictClass(id : string, name : string,keys: {key : string, value : string}[]):Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/ObjectType/PredictClass/`+id +'/'+name,keys);
}

AIModels(id : string):Observable<any> {
  return this.http.get<any>(`${this.baseUrl}/ObjectType/AIModels/`+id);
}
  
}
